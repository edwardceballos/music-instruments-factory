using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicInstrumentsCrm.Domain;
using MusicInstrumentsCrm.Repositories;

namespace MusicInstrumentsCrm
{
	public class Startup
	{
		private readonly SeedData seedData;

		private readonly string SpecificOrigins = "_specificOrigins";

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			seedData = new SeedData();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			services.AddDbContext<ApplicationDbContext>(
				options =>
				{
					var dbAddress = Environment.GetEnvironmentVariable("MICRM_DB_ADDRESS");
					options.UseNpgsql(
							$"Host={dbAddress ?? "192.168.99.100"};Database=micrm_db;Username=admin;Password=admin;Port=5432")
						.UseLazyLoadingProxies();
				});

			services.AddDefaultIdentity<User>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			services.AddIdentityServer()
				.AddApiAuthorization<User, ApplicationDbContext>();

			services.AddAuthentication()
				.AddIdentityServerJwt();

			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = true;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 3;
				options.Password.RequiredUniqueChars = 1;
				options.Password.RequireNonAlphanumeric = false;

				// Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				// User settings.
				options.User.AllowedUserNameCharacters =
					"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
				options.User.RequireUniqueEmail = false;
			});

			services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

				options.LoginPath = "/Identity/Account/Login";
				options.AccessDeniedPath = "/Identity/Account/AccessDenied";
				options.SlidingExpiration = true;
			});

			services.AddControllersWithViews();
			services.AddRazorPages();

			services.AddScoped<IGoodRepository, GoodRepository>();
			services.AddScoped<IGoodTypeRepository, GoodTypeRepository>();
			services.AddScoped<IGoodInOfferRepository, GoodInOfferRepository>();
			services.AddScoped<IFactoryRepository, FactoryRepository>();
			services.AddScoped<IOfferRepository, OfferRepository>();
			services.AddScoped<ISupplyInStoreRepository, SupplyInStoreRepository>();
			services.AddScoped<IStoreRepository, StoreRepository>();
			services.AddScoped<IBuyerRepository, BuyerRepository>();
			services.AddScoped<IStaffRepository, StaffRepository>();
			services.AddScoped<IDeliveryRepository, DeliveryRepository>();
			services.AddScoped<ICarRepository, CarRepository>();
			services.AddScoped<IMarkRepository, MarkRepository>();
			services.AddScoped<IModelRepository, ModelRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IAddressRepository, AddressRepository>();
			services.AddScoped<ICountryRepository, CountryRepository>();

			services.AddCors(options =>
			{
				options.AddPolicy(SpecificOrigins,
					builder =>
					{
						builder
							.AllowAnyHeader()
							.AllowAnyMethod()
							.AllowAnyOrigin();
					});
			});

			// In production, the React files will be served from this directory
			services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				seedData.Initialize(serviceProvider).Wait();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseCors(SpecificOrigins);

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapRazorPages();
			});
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseSpaStaticFiles();
			app.UseAuthentication();
			app.UseIdentityServer();

			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = "ClientApp";

				if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
			});
		}
	}
}