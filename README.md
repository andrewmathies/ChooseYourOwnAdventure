# Map
Originally I planned to build a "real world" mobile game inspired by choose your own adventure books. However just this demo took 40+ hours of work so I decided to stop after I met a few goals.

The app can be downloaded [here.](https://play.google.com/store/apps/details?id=com.awmathie.realWorldGame)
![alt-text](https://lh3.googleusercontent.com/uMl9zXVrjIRm05w_XgX-d_D5EMIpWM1hF8jtnGD6VlNchj9ZXiSpZ2bqJQkWOkWBZyA=w1600-h793-rw)

The app: 
1. Retrieves user location with GPS
2. Sends request to openstreetmap.org API to get XML map data of area around user
3. Parses XML map data stored in OSM format
4. Draws lines for 'ways' between 'nodes' of parsed data
5. Generates random GPS locations and places blue squares on the map denoting in-game locations

Because the amount of data being requested is fairly large, it may take up to 45 seconds for the map to be drawn. After the map is drawn you can use your fingers to zoom in/out and reposition the map. The controls weren't tuned so small gestures make large changes.
