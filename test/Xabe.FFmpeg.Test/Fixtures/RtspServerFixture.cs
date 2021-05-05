using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Xabe.FFmpeg.Test.Fixtures
{
    public class RtspServerFixture : IAsyncLifetime
    {
        private readonly DockerClient _dockerClient;
        private string _containerId;

        public RtspServerFixture()
        {
            _dockerClient = new DockerClientConfiguration().CreateClient();
        }

        public async Task DisposeAsync()
        {
            await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
        }

        public async Task InitializeAsync()
        {
            IList<ContainerListResponse> containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters());
            foreach (var container in containers.Where(x => x.Names.Contains("/Xabe.FFmpeg.Test")))
            {
                if (container.State == "running")
                {
                    await _dockerClient.Containers.StopContainerAsync(container.ID, new ContainerStopParameters());
                }
            }

            await _dockerClient.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = "aler9/rtsp-simple-server:latest"
                    },
                    null,
                    new Progress<JSONMessage>((m) => { }),
                    default);

            var response = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "aler9/rtsp-simple-server",
                ExposedPorts = new Dictionary<string, EmptyStruct>() { { "8554", default(EmptyStruct) } },
                Env = new List<string>() { "RTSP_PROTOCOLS=tcp" },
                HostConfig = new HostConfig()
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            {"8554", new List<PortBinding> {new PortBinding {HostPort = "8554" } }}
                        },
                    PublishAllPorts = true,
                    AutoRemove = true,
                },
                Name = "Xabe.FFmpeg.Test"
            });

            _containerId = response.ID;
            await _dockerClient.Containers.StartContainerAsync(_containerId, null);
        }

        public async Task Publish(string filePath, string name)
        {
            var parameters = $"-re -stream_loop -1 -i \"{filePath}\" -pix_fmt yuv420p -vsync 1 -vcodec libx264 -r 23.976 -threads 0 -b:v: 1024k -bufsize 1024k -preset veryfast -profile:v baseline -tune film -g 48 -x264opts no-scenecut -acodec aac -b:a 192k -f rtsp rtsp://127.0.0.1:8554/{name}";
            FFmpeg.Conversions.New().AddParameter(parameters).Start();
            await Task.Delay(2000);
        }
    }
}
