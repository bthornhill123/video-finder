# Simple YouTube Search UI

- A dynamic webpage where a user can type a search phrase into an input field and click the `Search` button
- UI requests are routed to a .NET Core Web API project which, subsequently, will make a web request to the Google API endpoint
- The results are parsed and returned to the client where a title, thumbnail, description, and tags are displayed for each video
- Clicking on a video thumbnail will open the video in a new tab
- After the initial 5 video results, a `Next` button allows for the requesting of an additional page of videos.

## Technologies

- HTML, CSS, JavaScript (keeping it simple)
- ASP.NET Web API,C#

## Resources

YouTube API Documentation

- Search/List: <https://developers.google.com/youtube/v3/docs/search/list>
- Videos/List: <https://developers.google.com/youtube/v3/docs/videos/list>
