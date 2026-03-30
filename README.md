# Finals_Q1

## Overview
Finals_Q1 is the backend API for Phase 2 of the Todo Management System.  
It is built with ASP.NET Core Web API and provides CRUD endpoints for managing todos.

## Tech Stack
- ASP.NET Core Web API
- C#
- In-memory data storage

## Features
- Get all todos
- Add a new todo
- Update an existing todo
- Delete a todo
- Input validation for empty titles
- CORS enabled for frontend connection

## API Endpoints

### GET /api/todos
Returns the list of todos.

### POST /api/todos
Creates a new todo.

Example request body:
```json
{
  "title": "Finish assignment",
  "completed": false
}