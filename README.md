[![Run in Postman](https://run.pstmn.io/button.svg)](https://app.getpostman.com/run-collection/22538867-7bce2470-b0ee-45a9-8207-e92afeb09616?action=collection%2Ffork&collection-url=entityId%3D22538867-7bce2470-b0ee-45a9-8207-e92afeb09616%26entityType%3Dcollection%26workspaceId%3D787de432-978a-4e40-8059-6af7d905d34b#?env%5BACE.Plugins%5D=W3sia2V5IjoidG9rZW4iLCJ2YWx1ZSI6IiIsImVuYWJsZWQiOnRydWUsInR5cGUiOiJhbnkiLCJzZXNzaW9uVmFsdWUiOiJleUpoYkdjaU9pSklVekkxTmlJc0luUjVjQ0k2SWtwWFZDSjkuZXlKb2RIUndPaTh2YzJOb1pXMWhjeTU0Yld4emIyRndMbTl5Wnk5M2N5OHlNREExTHpBMUwybGtaVzUwYVhSNUwyTnNZV2x0Y3k5dVlXMWxJam9pWVdSdGFXNGlMQ0pCWTJOdi4uLiIsInNlc3Npb25JbmRleCI6MH1d)

# ACE-Plugins
A collection of unofficial ACEmulator plugins.  ACE Plugins are instantiated and run from within the primary execution context of ACE.

* ACE.Plugin.Crypto - self-signing and startup services for certificates, data signing, certificate utilities.
* ACE.Plugin.Web - hosts a customizable website and various general API endpoints
* ACE.Plugin.Transfer - character backup/restore, interserver character migration, character transfers between accounts
    * Depends on ACE.Plugin.Crypto
    * Optionally depends on ACE.Plugin.Web
