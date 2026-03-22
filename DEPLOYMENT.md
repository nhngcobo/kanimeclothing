# SmarterASP.NET Deployment Setup

This project is configured for automatic deployment to SmarterASP.NET via GitHub Actions.

## Prerequisites

1. Your project must be pushed to GitHub
2. You must have GitHub Secrets configured in your repository

## Setting Up GitHub Secrets

To enable automatic deployment, follow these steps:

1. **Go to your GitHub repository settings**
   - Navigate to: Settings → Secrets and variables → Actions

2. **Create the following GitHub Secrets** (click "New repository secret"):

   - **SMARTERASP_MSDEPLOY_URL**
     - Value: `https://win8119.site4now.net:8172/msdeploy.axd?site=nhngcobo-001-site1`
     - (Found in your .PublishSettings file under `publishUrl`)

   - **SMARTERASP_MSDEPLOYSITENAME**
     - Value: `nhngcobo-001-site1`
     - (Found in your .PublishSettings file under `msdeploySite`)

   - **SMARTERASP_USERNAME**
     - Value: `nhngcobo-001`
     - (Your SmarterASP.NET account username)

   - **SMARTERASP_PASSWORD**
     - Value: `SheepTune@98`
     - (Your SmarterASP.NET account password)

3. **Commit and push the changes**
   ```powershell
   git add .github/workflows/deploy.yml
   git commit -m "Add GitHub Actions deployment workflow"
   git push origin main
   ```

## How It Works

The GitHub Actions workflow will:

1. ✅ Trigger automatically on every push to `main` or `master` branch
2. ✅ Set up .NET 10.0 environment
3. ✅ Restore dependencies
4. ✅ Build the project in Release mode
5. ✅ Publish the release build
6. ✅ Deploy to SmarterASP.NET using MSDeploy

## Manual Deployment

If you need to manually trigger a deployment:

1. Go to your GitHub repository
2. Click "Actions" tab
3. Select "Deploy to SmarterASP.NET" workflow
4. Click "Run workflow" → "Run workflow"

## Deployment URL

Once deployed, your site will be available at:
- **Primary:** http://nhngcobo-001-site1.ktempurl.com/

## Troubleshooting

### Authentication Failed
- Verify your GitHub Secrets match exactly with your SmarterASP.NET credentials
- Check that the `SMARTERASP_MSDEPLOY_URL` is correct

### Build Failed
- Check the workflow logs under Actions → [workflow run]
- Ensure the project builds locally: `dotnet build -c Release`

### Deployment Timeout
- SmarterASP.NET deployment can take 2-5 minutes
- The workflow will retry automatically

## Local Testing

Before pushing, test the build locally:

```powershell
dotnet restore
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

## Security Notes

⚠️ **Important:** The deployment credentials are stored as GitHub Secrets and are:
- ✅ Encrypted at rest
- ✅ Only available to the workflow
- ✅ Not visible in workflow logs
- ✅ Never committed to the repository

Do NOT commit the `.PublishSettings` file to GitHub.
