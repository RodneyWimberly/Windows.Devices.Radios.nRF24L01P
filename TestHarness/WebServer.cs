using HA4IoT.Networking;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Json;

namespace nRF24L01P.TestHarness
{
    public sealed class WebServer
{
    private BackgroundTaskDeferral _deferral;
    private HttpServer _httpServer;
    private IHttpRequestController _apiController;
    private readonly IReadAndWriteProcess _readAndWriteProcess;

    public WebServer(IReadAndWriteProcess readAndWriteProcess)
    {
        _readAndWriteProcess = readAndWriteProcess;
    }

    public void RunAsync(IBackgroundTaskInstance taskInstance)
    {
        _deferral = taskInstance.GetDeferral();
        Task.Factory.StartNew(this.InitializeHttpApi, TaskCreationOptions.LongRunning);
    }

    public void InitializeHttpApi()
    {
        _readAndWriteProcess.Initialize();

        _httpServer = new HttpServer();
        HttpRequestDispatcher httpRequestDispatcher = new HttpRequestDispatcher(_httpServer);
        _apiController = httpRequestDispatcher.GetController("api");
        _apiController.Handle(HttpMethod.Get, "read").Using(HandleApiRead);
        _apiController.Handle(HttpMethod.Post, "write").Using(HandleApiWrite);
        _httpServer.StartAsync(80).Wait();
    }

    private void HandleApiRead(HttpContext httpContext)
    {
        JsonObject activity = new JsonObject();
        byte[] bytes = _readAndWriteProcess.GetBytes();
        activity.SetNamedValue("data", JsonValue.CreateStringValue(Encoding.ASCII.GetString(bytes)));
        httpContext.Response.Body = new JsonBody(activity);
    }

    private void HandleApiWrite(HttpContext httpContext)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        JsonObject requestData;
        if (!JsonObject.TryParse(httpContext.Request.Body, out requestData))
        {
            httpContext.Response.StatusCode = HttpStatusCode.BadRequest;
            return;
        }
        _readAndWriteProcess.WriteBytes(new byte[] { 0 });

        JsonObject response = new JsonObject();
        response.SetNamedValue("ElapsedTime", JsonValue.CreateNumberValue(stopwatch.ElapsedMilliseconds));
        httpContext.Response.Body = new JsonBody(response);
    }
}
}
