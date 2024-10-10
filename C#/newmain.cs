using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace WarehouseApi
{
    public class ApiRequestHandler : HttpListener
    {
        public ApiRequestHandler(string prefix)
        {
            this.Prefixes.Add(prefix);
        }

        public void HandleGet(HttpListenerContext context, string[] path, User user)
        {
            if (!AuthProvider.HasAccess(user, path, "get"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
                return;
            }

            switch (path[0])
            {
                case "warehouses":
                    HandleWarehousesGet(context, path);
                    break;
                case "locations":
                    HandleLocationsGet(context, path);
                    break;
                case "transfers":
                    HandleTransfersGet(context, path);
                    break;
                case "items":
                    HandleItemsGet(context, path);
                    break;
                case "item_lines":
                    HandleItemLinesGet(context, path);
                    break;
                case "item_groups":
                    HandleItemGroupsGet(context, path);
                    break;
                case "item_types":
                    HandleItemTypesGet(context, path);
                    break;
                case "inventories":
                    HandleInventoriesGet(context, path);
                    break;
                case "suppliers":
                    HandleSuppliersGet(context, path);
                    break;
                case "orders":
                    HandleOrdersGet(context, path);
                    break;
                case "clients":
                    HandleClientsGet(context, path);
                    break;
                case "shipments":
                    HandleShipmentsGet(context, path);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                    break;
            }
        }

        private void HandleWarehousesGet(HttpListenerContext context, string[] path)
        {
            var paths = path.Length;
            switch (paths)
            {
                case 1:
                    var warehouses = DataProvider.FetchWarehousePool().GetWarehouses();
                    SendResponse(context, warehouses);
                    break;
                case 2:
                    var warehouseId = int.Parse(path[1]);
                    var warehouse = DataProvider.FetchWarehousePool().GetWarehouse(warehouseId);
                    SendResponse(context, warehouse);
                    break;
                case 3:
                    if (path[2] == "locations")
                    {
                        // Handle specific logic for "locations"
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        context.Response.Close();
                    }
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                    break;
            }
        }

        // Similar methods for HandleLocationsGet, HandleTransfersGet, HandleItemsGet, etc.

        private void SendResponse(HttpListenerContext context, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var buffer = Encoding.UTF8.GetBytes(json);
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        public void HandlePost(HttpListenerContext context, string[] path, User user)
        {
            if (!AuthProvider.HasAccess(user, path, "post"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
                return;
            }

            // Handle POST logic based on path
        }

        public void HandlePut(HttpListenerContext context, string[] path, User user)
        {
            if (!AuthProvider.HasAccess(user, path, "put"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
                return;
            }

            // Handle PUT logic based on path
        }

        public void HandleDelete(HttpListenerContext context, string[] path, User user)
        {
            if (!AuthProvider.HasAccess(user, path, "delete"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.Close();
                return;
            }

            // Handle DELETE logic based on path
        }

        public void Start()
        {
            this.Start();
            Console.WriteLine("Serving on port 3000...");
            while (true)
            {
                var context = this.GetContext();
                var apiKey = context.Request.Headers["API_KEY"];
                var user = AuthProvider.GetUser(apiKey);
                if (user == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.Close();
                }
                else
                {
                    var path = context.Request.Url.AbsolutePath.Trim('/').Split('/');
                    switch (context.Request.HttpMethod)
                    {
                        case "GET":
                            HandleGet(context, path, user);
                            break;
                        case "POST":
                            HandlePost(context, path, user);
                            break;
                        case "PUT":
                            HandlePut(context, path, user);
                            break;
                        case "DELETE":
                            HandleDelete(context, path, user);
                            break;
                        default:
                            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                            context.Response.Close();
                            break;
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            AuthProvider.Init();
            DataProvider.Init();
            NotificationProcessor.Start();

            var handler = new ApiRequestHandler("http://localhost:3000/");
            handler.Start();
        }
    }
}