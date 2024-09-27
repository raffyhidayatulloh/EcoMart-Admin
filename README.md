Ecomart is a web admin platform designed for managing e-commerce operations, making it easier to handle products, orders, categories, users, and other essential features of an e-commerce system. Built with modern technologies, Ecomart offers an intuitive and efficient interface for administrators.

1. Go into directory where you plan on keeping project and run.

```bash
  git fork https://github.com/raffyhidayatulloh/EcoMart-Admin.git
```

2. Create a local database.

3. Add connection string to app settings.json. It will look something like this:
```bash
  Data Source=RAFFY\\SQLEXPRESS;Initial Catalog=EcoMart;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False
```
4. Register for a Cloudinary Account and add Cloudname, ApiKey, and Api secret to appsettings.json.
