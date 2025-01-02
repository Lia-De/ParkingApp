# ParkingApp
1. Purpose
Control an app’s inner working through an API.
2. Assignment
This is a mandatory assignment you will continue with during the
Christmas weeks. Share a link to your code on GitHub with me on Thursday
in the first week of 2025.
We will return to this app during the course and improve upon it. Consider
making it a single standalone repository on GitHub so that it becomes easy
to include as a demonstation project in your profile material (like CV.)
Use Swagger to test the endpoints of your app. Swagger can work as a
form of prototype frontend solution. You can add HTML pages for POST
methods, but that’s optional. Eventually, we will use JavaScript to power
the frontend. Then we will be able to send application/json as content-type
to our backend. So, your POST methods should be prepared for this,
accepting application/json.
Imagine the app being accessed through a mobile, with a simple user
interface as a consequence. Your backend provides a number of API
endpoint that makes possible a future frontend solution. You have to
imagine the frontend through your endpoints.
The practical circumstances for our client – one parking area with an hourly
charge 14 SEK between 8 and 18 and 6 SEK the rest of the day. Users have
an account that collects the cost. At some point the user is charged for
their costs, but that is outside of the app at this stage.
Endpoints:
• begin a new period (GET or POST)
• end the present period (GET or POST)
• get the present period for a car (GET)
• register a user/car (POST)
• get cost on registered account (GET)
• get user’s all registered details (GET)
The backend needs to keep track of time in order to calculate costs.
