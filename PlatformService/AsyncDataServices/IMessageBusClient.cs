﻿using PlatformService.Dtos;

namespace PlatformService.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PulishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}
