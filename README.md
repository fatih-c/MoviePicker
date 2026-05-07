# MoviePicker

A real-time multiplayer movie selection app built with ASP.NET Core MVC. A group of people join a shared room, swipe through movies, and the app reveals which movies everyone liked.

## How It Works

1. A creator opens a room and sets movie filters
2. Others join using a shared room code and a temporary username
3. Everyone swipes through the same list of movies
4. When all members finish, the app shows movies that everyone liked, sorted by total likes

## Features

- Solo mode: filter and swipe through movies without a room
- Multiplayer rooms with a shareable 6-character code
- Real-time lobby updates when members join, powered by SignalR
- Synchronized swiping: all members see the same movie list
- Results ranked by number of likes across all members
- Movie details modal with poster, rating, release year, and synopsis
- Room data is deleted from the database after results are shown

## Tech Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8 with PostgreSQL
- SignalR for real-time communication
- TMDB API for movie data
- Vanilla JavaScript and CSS
