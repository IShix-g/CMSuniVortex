
# Google Sheetの初期設定

## 設定の流れ

- [Google Cloud](https://console.cloud.google.com/)でプロジェクトを作成
- Google Sheets API	/ Google Drive APIの有効化
- サービスアカウントの作成
- サービスアカウント キーの作成
- スプレッドシートの共有設定

## Google Cloudでプロジェクトを作成

[Google Cloud](https://console.cloud.google.com/)を開いて赤枠をクリック

![](assets/googleSheet/top.png)

プロジェクトを作成します。

![](assets/googleSheet/create_project.png)

「APIとサービス」をクリック

![](assets/googleSheet/api_click.png)

## APIの有効化

2つのAPIを有効化します。

- Google Sheets API
- Google Drive API

![](assets/googleSheet/enable_api.png)

## サービスアカウントの作成

設定 > IAMと管理 > サービスアカウントをクリック

![](assets/googleSheet/service_account.png)

サービスアカウントを作成します。

![](assets/googleSheet/service_account_create.png)

名前とIDを入力します。

![](assets/googleSheet/service_account_create2.png)

ロールは「閲覧者」を選択してください。

![](assets/googleSheet/service_account_create3.png)

## サービスアカウントキーの作成

生成したサービスアカウントをクリック。 生成されるメールアドレスは後ほどDriveの設定で使います。

![](assets/googleSheet/service_account_key.png)

新しい鍵を作成してください。

![](assets/googleSheet/service_account_key2.png)

タイプをJSONにして保存します。 Unityの適当な場所に配置してください。
また、このファイルはバージョン管理に含めないように注意してください。

![](assets/googleSheet/service_account_key3.png)

## スプレッドシートの共有設定

[Google Drive](https://drive.google.com/drive/home)に移動し、インポート用に使うスプレドシートが無ければ作成してください。ファイルの共有設定を開きます。

![](assets/googleSheet/drive_share.png)

ユーザーを追加から上記で取得したサービスアカウントのメールアドレスを設定します。 ロールを閲覧者を選択してください。また、今回はスプレッドシートにロールを設定しましたが、フォルダに設定してフォルダ配下すべてに許可を追加する事も可能です。

![](assets/googleSheet/drive_share2.png)

以上で設定は完了です。

## サービスアカウントキーの管理

**jsonファイルは絶対に外部に公開しないでください。** もちろんGitHubにアップしてもいけません。
複数人で開発する場合は、インポート担当を決めるか安全な方法で相手に送信してください。
できれば、それぞれがGoogle Cloudにログインしてサービスアカウントキーを生成するのがベストだと思います。