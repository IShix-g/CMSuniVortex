
[日本語](InitialSetupOfGoogleSheet_jp.md)

# Initial Setup of Google Sheet

## Setup Flow

- Create a Project in [Google Cloud](https://console.cloud.google.com/)
- Enable Google Sheets API and Google Drive API
- Create a Service Account
- Create a Service Account Key
- Set the Sharing Settings for the Spreadsheet

## Create a Project in Google Cloud

Open [Google Cloud](https://console.cloud.google.com/) and click on the highlighted box

![](assets/googleSheet/top.png)

Create a project.

![](assets/googleSheet/create_project.png)

Click on "APIs and Services".

![](assets/googleSheet/api_click.png)

## Enable API

Enable two APIs:

- Google Sheets API
- Google Drive API

![](assets/googleSheet/enable_api.png)

## Creating a Service Account

Go to Settings > IAM and Admin > Service Accounts

![](assets/googleSheet/service_account.png)

Create a Service Account.

![](assets/googleSheet/service_account_create.png)

Enter the name and ID.

![](assets/googleSheet/service_account_create2.png)

Please select “Viewer” for Role.

![](assets/googleSheet/service_account_create3.png)

## Creating a Service Account Key

Click on the created Service Account. The generated email address will be used for Drive Settings later.

![](assets/googleSheet/service_account_key.png)

Please create a new key.

![](assets/googleSheet/service_account_key2.png)

Set the type to JSON and save it. Place it in an appropriate place in Unity.
Also, make sure this file is not included in version control.

![](assets/googleSheet/service_account_key3.png)

## Spreadsheet Sharing Settings

Go to [Google Drive](https://drive.google.com/drive/home), and create a spreadsheet if you haven't done so for importing. Open the file sharing settings.

![](assets/googleSheet/drive_share.png)

From Add User, set the email address of the Service Account you acquired. Select Viewer for Role. Also, while we have set the role for the spreadsheet this time, it is possible to set permissions for an entire folder by adding roles to it.

![](assets/googleSheet/drive_share2.png)

This completes the settings.

## Managing the Service Account Key

**Never disclose the json file externally.** Of course, you should not upload it to GitHub.
If you are developing with multiple people, decide who will handle the import or send it to the other person in a secure way.
Ideally, each person should log into Google Cloud and generate a Service Account Key.