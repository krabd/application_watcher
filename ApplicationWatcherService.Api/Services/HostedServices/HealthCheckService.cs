﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationWatcherService.Api.Models.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApplicationWatcherService.Api.Services.HostedServices
{
    public class HealthCheckService : BackgroundService
    {
        private readonly ISvGrpcClientService _svGrpcClientService;
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly GrpcOptions _grpcOptions;
        private readonly ILogger<HealthCheckService> _logger;

        public HealthCheckService(ISvGrpcClientService svGrpcClientService, IOptions<HealthCheckOptions> healthCheckOptions, IOptions<GrpcOptions> grpcOptions,
            ILogger<HealthCheckService> logger)
        {
            _svGrpcClientService = svGrpcClientService;

            _healthCheckOptions = healthCheckOptions.Value;
            _grpcOptions = grpcOptions.Value;

            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Heath check service running check");

                try
                {
                    await _svGrpcClientService.HealthCheckSv(_grpcOptions.GetUri(), _healthCheckOptions.TimeoutSeconds, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Heath check service error check", e);
                }

                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
