# WindowsInfoParser
Computer management tool. It is used to make endpoints dump basic info about their configuration on a specified place. It works with calls, which are simply folders were clients will have to deposit their configuration. It can then export all those answers to a CSV or a JSON array for further processing.

## Calls
Calls are folder with a date as a name. Clients will crwal a root folder to find such folders and take the most recent folder (as announced by is name) that is not in the future. It will then dump is information in this selected folder.
You can create a call using `WindowsInfoGatherer create --Date 2018-03-05 --folder D:\test\Calls\` for example.

You can use this to create several calls in a future, say one every two weeks. You will then have an inventory of your infratructure updated at the pace you wish.

## Answering calls
To answer call, a client will crawl the root folder and select the appropriate call. It will then check if it has already answer by looking if a file corresponding to its name already exists. If not, it will create it and write its configuration with JSON.
You can answer a call by using `WindowsInfoGatherer answer --folder D:\test\Calls\`

## Exporting calls
To gather all configurations dumped in a call folder, you can use `WindowsInfoGatherer export --folder D:\test\Calls\2018-02-21 --out "D:\test\Calls\2018-02-21 Report.csv" --type Csv`. Possible types are Csv and Json, casing seems to matter. It will create a file with the informations gathered in the specified format.

### Note

The application needs admin rights once to create the source in the event log. If it is not created and the application is not admin, it will simply not log in the event logs.
