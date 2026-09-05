# frontend — React + Vite + TypeScript

Client UI for the Consultora API (paquetes / consultores / reportes).

## Stack

- Vite 8 + React 19 + TypeScript
- Material UI
- React Router (protected routes)
- axios (centralized `src/services/api.ts` client, bearer token + 401 handling)
- Vitest + React Testing Library

## Scripts

```bash
pnpm install   # install dependencies
pnpm dev       # start dev server (http://localhost:5173)
pnpm build     # typecheck + production build
pnpm test      # run unit/component tests
pnpm lint      # oxlint
```

## Configuration

Copy `.env.example` to `.env` and adjust the API base URL:

```
VITE_API_URL=http://localhost:5058/api/v1
```

See the repository root `README.md` for full setup, credentials and conventions.