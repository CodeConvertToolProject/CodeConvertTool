# CodeConvertTool

## How to run server

1. Start the server in Visual Studio or VS Code using the `run` option or alternatively run `dotnet build` then `dotnet run`
2. Server will be running on `https://localhost:<PORT>`
3. visit `https://localhost:<PORT>/swagger/index.html` to for API endpoints documentation
4. To get bearer access token, call the endpoint: `<base_url>/api/Login/InitiateLogin` with required params for OAuth Login
5. The call `<base_url>/api/Login/PollForAccessCode` to get access token
6. You are now free to make use of the API endpoints
