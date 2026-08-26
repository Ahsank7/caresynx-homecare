# CareSynX Home Care

Home Care scheduling platform — React UI + .NET 8 API. Part of the CareSynX product family.

## URLs

| URL | Component |
|-----|-----------|
| https://homecare.caresynx.com | React app (login, dashboard, billing, etc.) |
| https://api.homecare.caresynx.com | .NET API + Swagger |

Company marketing site: [caresynx-marketing](https://github.com/Ahsank7/caresynx-marketing) → https://caresynx.com

## Repository structure

```text
caresynx-homecare/
|-- Scheduler.API/                    # Home Care backend
|   `-- deploy/nginx-api.homecare.caresynx.com.conf
|-- Scheduler.Client/eXtream-scheduler/  # Home Care frontend
|   `-- deploy/nginx-homecare.caresynx.com.conf
|-- Scheduler.DB/
|-- Scheduler.MCP/                    # MCP bridge (legacy EC2 deploy)
|-- .github/workflows/
|   |-- deploy-api.yml
|   |-- deploy-ui.yml
|   `-- deploy-mcp.yml
`-- docs/VPS-MULTI-PRODUCT-CUTOVER.md
```

## Deployment

| Workflow | Target |
|----------|--------|
| **Deploy Backend API to VPS** | `/opt/caresynx/publish` → `api.homecare.caresynx.com` |
| **Deploy Home Care UI to VPS** | `/var/www/homecare` → `homecare.caresynx.com` |
| **Deploy MCP server to EC2** | Legacy MCP host |

First-time VPS setup: **[SETUP.md](SETUP.md)** and **[docs/VPS-MULTI-PRODUCT-CUTOVER.md](docs/VPS-MULTI-PRODUCT-CUTOVER.md)**

## GitHub secrets (required)

| Name | Purpose |
|------|---------|
| `VPS_HOST` | VPS IP |
| `VPS_SSH_KEY` or `VPS_SSH_KEY_B64` | Deploy SSH key |

## GitHub variables (optional)

| Name | Default |
|------|---------|
| `VPS_USER` | `root` |
| `REACT_APP_API_URL` | `https://api.homecare.caresynx.com/api/` |
| `VPS_UI_DIR` | `/var/www/homecare` |
| `VPS_UI_HEALTHCHECK_URL` | `https://homecare.caresynx.com/` |
| `VPS_HEALTHCHECK_URL` | `https://api.homecare.caresynx.com/swagger/index.html` |

## Related repos

- [caresynx-marketing](https://github.com/Ahsank7/caresynx-marketing) — caresynx.com company site
