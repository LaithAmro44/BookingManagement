# Booking Management UI

A modular Angular 14 frontend for the Booking Management API.

## Compatibility

- Node.js: `16.20.2`
- npm: `8.x`
- Angular: `14.2.12`
- Angular CLI: local project CLI `14.2.12`

## Important

Run commands from the folder that contains **package.json** and **angular.json** — not from `src`.

The project intentionally does **not** include `node_modules` or `package-lock.json`.
`package-lock.json` is generated locally by npm so it uses your public npm registry, not an internal registry.

## Install and run

```powershell
npm install --legacy-peer-deps --no-audit --no-fund
npm start
```

Open: `http://localhost:4200`

## API address

`proxy.conf.json` proxies `/api` to:

```text
https://localhost:7001
```

Change that target only if your API is running on another HTTPS port.

## Pages

- `/dashboard`
- `/bookings`
- `/bookings/new`
- `/resources`
- `/users`

## Main structure

```text
src/app/
  core/                 API service, models, error handling
  layout/               Application shell, sidebar, top bar
  shared/components/    Reusable components
  features/
    dashboard/
    bookings/
      components/
      pages/
    reference-data/
```
