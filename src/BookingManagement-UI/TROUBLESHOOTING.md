# Troubleshooting

## Error: Cannot find type definition file for `jasmine`

This project has no unit-test setup, so it intentionally does not depend on Jasmine.

If the error came from an older extracted copy, remove its `node_modules` and `package-lock.json`, then use this corrected project folder:

```powershell
Remove-Item -Recurse -Force node_modules -ErrorAction SilentlyContinue
Remove-Item -Force package-lock.json -ErrorAction SilentlyContinue
npm cache clean --force
npm install --legacy-peer-deps --no-audit --no-fund
npm start
```

Run these commands in the folder containing `package.json`, not inside `src`.
