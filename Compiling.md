# Compiling from Source

1. Use git to clone the repo `git clone https://github.com/LethalCompanyModding/ShipInventoryCommunity.git`
2. Restore the project and tools `dotnet tool restore; dotnet restore`
3. Perform a test build, if you use VS:Code then use the F1 key with the project open and select Run Task -> Build LCM Project. Otherwise use your IDE's build command or `dotnet build` in the root of the project

## Considerations

If you are running `VS:Code` I have provided a few extra tasks you can run from the Run Task menu:

- **Build LCM Project**: This will build the project with a few switches set to avoid common problems
- **Dry-run Package Test**: This will preview how the package will look uploaded to the thunderstore, you need to run `dotnet publish` first and ignore the error about not having an auth token (you're not supposed to have one). The resulting zip will use dummy names for the project and author and is simply to check that all files are being copied to the zip correctly.

The package test task should be usable from within `VS Studio` as well but I can't test it
