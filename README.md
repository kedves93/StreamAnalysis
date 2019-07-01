# StreamAnalysis

A cloud hosted web framework that lets users visualize their streamed data. The user pushes a Docker image to the application's registry which is then ran or scheduled to ran on his command. This image has to contain an application that streams information into the application's broker. Container data is then retrieved and plotted.