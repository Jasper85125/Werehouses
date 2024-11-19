// using System;
// using System.IO;
// using System.Net;
// using System.Text;
// using Newtonsoft.Json;

// namespace WarehouseApi
// {
//     public static class AuthProvider
//     {
//         public static void Init()
//         {
//             // Initialization logic for AuthProvider
//         }

//         public static bool HasAccess(User user, string[] path, string method)
//         {
//             // Access control logic
//             return true;
//         }

//         public static User GetUser(string apiKey)
//         {
//             // Logic to get user by API key
//             return new User();
//         }
//     }

//     public class User
//     {
//         // User class definition
//     }
//     public static class NotificationProcessor
//     {
//         public static void Start()
//         {
//             // Initialization logic for NotificationProcessor
//             Console.WriteLine("Notification Processor Started.");
//         }
//     }

//     public static class DataProvider
//     {
//         public class WarehousePool
//         {
//             public object GetWarehouses()
//             {
//                 // Logic to get warehouses
//                 return new object();
//             }

//             public object GetWarehouse(int warehouseId)
//             {
//                 // Logic to get a specific warehouse
//                 return new object();
//             }
//         }

//         public static void Init()
//         {
//             // Initialization logic for DataProvider
//         }

//         public static WarehousePool FetchWarehousePool()
//         {
//             // Logic to fetch the warehouse pool
//             return new WarehousePool();
//         }

//         // Other methods and properties for DataProvider
//     }

//     public class ApiRequestHandler
//     {
//         private readonly HttpListener _listener;

//         public ApiRequestHandler(string prefix)
//         {
//             _listener = new HttpListener();
//             _listener.Prefixes.Add(prefix);
//         }

//         public void HandleGet(HttpListenerContext context, string[] path, User user)
//         {
//             if (!AuthProvider.HasAccess(user, path, "get"))
//             {
//                 context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                 context.Response.Close();
//                 return;
//             }

//             switch (path[0])
//             {
//                 case "warehouses":
//                     HandleWarehousesGet(context, path);
//                     break;
//                 case "locations":
//                     HandleLocationsGet(context, path);
//                     break;
//                 case "transfers":
//                     HandleTransfersGet(context, path);
//                     break;
//                 case "items":
//                     HandleItemsGet(context, path);
//                     break;
//                 case "item_lines":
//                     HandleItemLinesGet(context, path);
//                     break;
//                 case "item_groups":
//                     HandleItemGroupsGet(context, path);
//                     break;
//                 case "item_types":
//                     HandleItemTypesGet(context, path);
//                     break;
//                 case "inventories":
//                     HandleInventoriesGet(context, path);
//                     break;
//                 case "suppliers":
//                     HandleSuppliersGet(context, path);
//                     break;
//                 case "orders":
//                     HandleOrdersGet(context, path);
//                     break;
//                 case "clients":
//                     HandleClientsGet(context, path);
//                     break;
//                 case "shipments":
//                     HandleShipmentsGet(context, path);
//                     break;
//                 default:
//                     context.Response.StatusCode = (int)HttpStatusCode.NotFound;
//                     context.Response.Close();
//                     break;
//             }
//         }

//         private void HandleWarehousesGet(HttpListenerContext context, string[] path)
//         {
//             var paths = path.Length;
//             switch (paths)
//             {
//                 case 1:
//                     var warehouses = DataProvider.FetchWarehousePool().GetWarehouses();
//                     SendResponse(context, warehouses);
//                     break;
//                 case 2:
//                     var warehouseId = int.Parse(path[1]);
//                     var warehouse = DataProvider.FetchWarehousePool().GetWarehouse(warehouseId);
//                     SendResponse(context, warehouse);
//                     break;
//                 case 3:
//                     if (path[2] == "locations")
//                     {
//                         // Handle specific logic for "locations"
//                     }
//                     else
//                     {
//                         context.Response.StatusCode = (int)HttpStatusCode.NotFound;
//                         context.Response.Close();
//                     }
//                     break;
//                 default:
//                     context.Response.StatusCode = (int)HttpStatusCode.NotFound;
//                     context.Response.Close();
//                     break;
//             }
//         }

//         private void HandleLocationsGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling locations GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleTransfersGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling transfers GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleItemsGet(HttpListenerContext context, string[] path)
//         {
//             Console.WriteLine("HandleItemsGet called with path: " + string.Join("/", path));
//             var itemsCS = new ItemsCS("models");
//             var items = itemsCS.GetItemsCS();
//             SendResponse(context, items);
//         }

//         private void HandleItemLinesGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling item lines GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleItemGroupsGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling item groups GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleSuppliersGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling suppliers GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleClientsGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling clients GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleShipmentsGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling shipments GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleOrdersGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling orders GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         // Similar methods for HandleItemsGet, etc.

//         private void HandleInventoriesGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling inventories GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void HandleItemTypesGet(HttpListenerContext context, string[] path)
//         {
//             // Implement the logic for handling item types GET request
//             context.Response.StatusCode = (int)HttpStatusCode.OK;
//             context.Response.Close();
//         }

//         private void SendResponse(HttpListenerContext context, object data)
//         {
//             var json = JsonConvert.SerializeObject(data);
//             var buffer = Encoding.UTF8.GetBytes(json);
//             context.Response.ContentType = "application/json";
//             context.Response.ContentLength64 = buffer.Length;
//             context.Response.OutputStream.Write(buffer, 0, buffer.Length);
//             context.Response.Close();
//         }

//         public void HandlePost(HttpListenerContext context, string[] path, User user)
//         {
//             if (!AuthProvider.HasAccess(user, path, "post"))
//             {
//                 context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                 context.Response.Close();
//                 return;
//             }

//             // Handle POST logic based on path
//         }

//         public void HandlePut(HttpListenerContext context, string[] path, User user)
//         {
//             if (!AuthProvider.HasAccess(user, path, "put"))
//             {
//                 context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                 context.Response.Close();
//                 return;
//             }

//             // Handle PUT logic based on path
//         }

//         public void HandleDelete(HttpListenerContext context, string[] path, User user)
//         {
//             if (!AuthProvider.HasAccess(user, path, "delete"))
//             {
//                 context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
//                 context.Response.Close();
//                 return;
//             }
//         }

//         public void Start()
//         {
//             _listener.Start();
//             while (true)
//             {
//                 var context = _listener.GetContext();
//                 var apiKey = context.Request.Headers["API_KEY"];
//                 if (string.IsNullOrEmpty(apiKey))
//                 {
//                     context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//                     context.Response.Close();
//                 }
//                 else
//                 {
//                     var user = AuthProvider.GetUser(apiKey);
//                     if (user == null)
//                     {
//                         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                         context.Response.Close();
//                     }
//                     else
//                     {
//                         var url = context.Request.Url;
//                         if (url == null)
//                         {
//                             context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//                             context.Response.Close();
//                             continue;
//                         }
//                         var path = url.AbsolutePath.Trim('/').Split('/');
//                         switch (context.Request.HttpMethod)
//                         {
//                             case "GET":
//                                 HandleGet(context, path, user);
//                                 break;
//                             case "POST":
//                                 HandlePost(context, path, user);
//                                 break;
//                             case "PUT":
//                                 HandlePut(context, path, user);
//                                 break;
//                             case "DELETE":
//                                 HandleDelete(context, path, user);
//                                 break;
//                             default:
//                                 context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
//                                 context.Response.Close();
//                                 break;
//                         }
//                     }
//                 }
//             }
//         }

//         public static void Main(string[] args)
//         {
//             AuthProvider.Init();
//             DataProvider.Init();
//             NotificationProcessor.Start();

//             var handler = new ApiRequestHandler("http://localhost:3000/");
//             handler.Start();
//         }
//     }
// }