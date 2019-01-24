# Custom Auth Attribute

## Tech :robot:

- .Net Core 2.2

## Features :sparkles:

- Cookie & query string authentication
  - Allow users to authenticate and maintain sessions using both cookies and query strings
  - Enables multi user access within the same browser

## Example usage

- Cookie user can login using http://localhost:5000/api/authentication/login
- Logged in user should have access to all routes
- Query string user should only have access to Value/{id} route

### Route access

| User Type    | Route                                            | Authorised |
| ------------ | ------------------------------------------------ | ---------- |
| Cookie       | http://localhost:5000/api/values                 | ✅         |
| Cookie       | http://localhost:5000/api/values/3               | ✅         |
| Query string | http://localhost:5000/api/values?session=56837   | ❌         |
| Query string | http://localhost:5000/api/values/3?session=56837 | ✅         |
