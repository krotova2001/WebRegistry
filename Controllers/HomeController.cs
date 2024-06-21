using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using WebRegistry.Models;
using WebRegistry.Services;

namespace WebRegistry.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
    public async Task<string> Test()
    {
        var _1C = new _1CService(new HttpClient());
        var res = await _1C.GetAll();
        var json = JsonConvert.SerializeObject(res);
       
        return json;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}