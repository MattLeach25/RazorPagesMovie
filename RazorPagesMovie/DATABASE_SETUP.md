# SQLite Database Setup - COMPLETE ?

## What Was Done

Your app has been successfully configured to use **SQLite** instead of SQL Server!

### Changes Made:
1. ? Installed `Microsoft.EntityFrameworkCore.Sqlite` package (v9.0.0)
2. ? Updated `appsettings.json` connection string to: `"Data Source=RazorPagesMovie.db"`
3. ? Changed `Program.cs` from `UseSqlServer` to `UseSqlite`
4. ? Added automatic database migration on app startup
5. ? Removed old SQL Server migrations
6. ? Created fresh SQLite migration

## How to Run Your App

### Option 1: Automatic (Recommended) ?
Just **start your app** normally - the database will be created automatically!

The code in `Program.cs` will:
- Create `RazorPagesMovie.db` in your project folder
- Create the Movie table
- Apply all migrations

### Option 2: Manual Database Creation
If you want to manually create the database first:

```powershell
cd C:\Users\leachmatthew\source\repos\RazorPagesMovie\RazorPagesMovie
dotnet ef database update
```

## Database Location

The SQLite database file will be created here:
```
C:\Users\leachmatthew\source\repos\RazorPagesMovie\RazorPagesMovie\RazorPagesMovie.db
```

## Troubleshooting

### If you still get "no table Movie" error:

1. **Stop your app completely** (if running)
2. **Delete the database file** (if it exists):
   ```powershell
   cd C:\Users\leachmatthew\source\repos\RazorPagesMovie\RazorPagesMovie
   Remove-Item RazorPagesMovie.db -ErrorAction SilentlyContinue
   Remove-Item RazorPagesMovie.db-shm -ErrorAction SilentlyContinue
   Remove-Item RazorPagesMovie.db-wal -ErrorAction SilentlyContinue
   ```
3. **Start the app again** - the database will be recreated with the correct table

### To view your SQLite database:
- Use **DB Browser for SQLite** (free): https://sqlitebrowser.org/
- Or use Visual Studio extension: **SQLite/SQL Server Compact Toolbox**

## Benefits of SQLite

? **No installation required** - works immediately  
? **Single file database** - easy to backup/delete  
? **Perfect for development** - lightweight and fast  
? **Cross-platform** - works on Windows, Mac, Linux  

## Next Steps

Your app is ready to run! Just press F5 or start debugging. The database will be created automatically on first run.
