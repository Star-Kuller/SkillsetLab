using System.Diagnostics;
using System.IO.Compression;
using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Features.Tasks;

public class TaskCreate
{
    public class Command : IRequest<long>
    {
        public IFormFile ZipFile { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, long>
    {
        private IMyDbContext _context;
        public Handler(IMyDbContext context)
        {
            _context = context;
        }

        public async Task<long> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.ZipFile == null || request.ZipFile.Length == 0)
                throw new ValidationException(nameof(Command.ZipFile), "Файл не найден");

            var tempPath = Path.GetTempPath();
            var filePath = Path.Combine(tempPath, request.ZipFile.FileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.ZipFile.CopyToAsync(stream);
            }
            
            var extractPath = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(request.ZipFile.FileName));
            ZipFile.ExtractToDirectory(filePath, extractPath);

            var processDockerBuildInfo = new ProcessStartInfo("docker", $"build -t project_name {extractPath}");
            var processDockerBuild = Process.Start(processDockerBuildInfo);
            var task = processDockerBuild.WaitForExitAsync(cancellationToken);

            var processDockerRunInfo = new ProcessStartInfo("docker", "run -d -p 8080:80 project_name");
            var processDockerRun = Process.Start(processDockerRunInfo);
            await processDockerRun.WaitForExitAsync(cancellationToken);

            if (processDockerRun.ExitCode != 0)
                throw new Exception("Тесты провалены");
            
            _context.Task.Add(task);
            return task.Id;
        }
    }
}