This folder used for storing language packs to support more languages, or modify (but default language pack can't be modified).

Language pack content should be JSON format, for more information: https://en.wikipedia.org/wiki/JSON

Template of language pack content:
{
    "name": "Language name",
    "i18nName": "en-US",
    "description": "You can leave a comment for your language pack (But it invisible now, implement of description will later).",
    "creator": "Even your name (or nickname, whatever. But recommend leave your social nickname).",
    "table": {
        // And there we go, a list of string items. It have two parts,
        // one for define item id (or key) and second is value of this item.
        "common.cancel": "Cancel",
        "common.ok": "OK",
        "common.rename": "Rename",
        // .. etc. For more items changes, you can check the program code FallbackLanguage.cs on this folder project.
		// or start application with argument /outputlangpack to get all nodes from fallback language pack.
    }
}