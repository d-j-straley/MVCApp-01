var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); //   this enables session state
builder.Services.AddHttpContextAccessor(); // also necessary for session state to be enabled

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
// Note that although we are specifying traditional routing methodolgy here, 
// we are also using attribute routing in the ContactsController.cs file,
// so we are mixing and matching routing methodologies, because MVC allows this
// flexibility by default.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
