using System.Diagnostics;
using System.IO.Compression;
using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Features.Tasks;

public class TaskTest
{
    public class Command : IRequest<bool>
    {
        public IFormFile ZipFile { get; set; }
        public long Id { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, bool>
    {
        private IMyDbContext _context;
        public Handler(IMyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
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

            var processDockerBuildInfo = new ProcessStartInfo("docker", $"build -t your_project_name {extractPath}");
            var processDockerBuild = Process.Start(processDockerBuildInfo);
            await processDockerBuild.WaitForExitAsync(cancellationToken);

            var processDockerRunInfo = new ProcessStartInfo("docker", "run -d -p 8080:80 your_project_name");
            var processDockerRun = Process.Start(processDockerRunInfo);
            await processDockerRun.WaitForExitAsync(cancellationToken);

            var tests = _context.Task.FirstOrDefault(x => x.Id == request.Id);
            
            var processTestsDockerBuildInfo = new ProcessStartInfo("docker", $"build -t your_project_name {tests.Id}");
            var processTestsDockerBuild = Process.Start(processTestsDockerBuildInfo);
            await processTestsDockerBuild.WaitForExitAsync(cancellationToken);

            var processTestsDockerRunInfo = new ProcessStartInfo("docker", "run -d -p 8080:80 your_project_name");
            var processTestsDockerRun = Process.Start(processTestsDockerRunInfo);
            await processTestsDockerRun.WaitForExitAsync(cancellationToken);
            
            return processTestsDockerRun.ExitCode == 0;
        }
    }
}