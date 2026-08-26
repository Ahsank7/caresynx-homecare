# Manual setup — Home Care product (UI + API)

## 1. Create GitHub repo

```bash
cd caresynx-homecare
git init
git add .
git commit -m "Initial CareSynX Home Care product"
git branch -M main
git remote add origin https://github.com/Ahsank7/caresynx-homecare.git
git push -u origin main
```

Create the empty repo on GitHub first if it does not exist.

## 2. GitHub Actions secrets

In **caresynx-homecare** repo → Settings → Secrets and variables → Actions:

| Name | Value |
|------|--------|
| `VPS_HOST` | Your VPS IP |
| `VPS_SSH_KEY` or `VPS_SSH_KEY_B64` | SSH private key |
| `VPS_CONNECTION_STRING` | SQL Server connection string (optional) |

### Recommended variables

| Name | Value |
|------|--------|
| `VPS_USER` | `root` |
| `REACT_APP_API_URL` | `https://api.homecare.caresynx.com/api/` |

## 3. GoDaddy DNS

| Type | Name | Value |
|------|------|--------|
| A | `homecare` | VPS IP |
| A | `api.homecare` | VPS IP |

Remove the old **`api`** A record (`api.caresynx.com`) if still present.

Marketing DNS (`@`, `www`) is configured in the **caresynx-marketing** repo setup.

## 4. VPS — Nginx + TLS

See **[docs/VPS-MULTI-PRODUCT-CUTOVER.md](docs/VPS-MULTI-PRODUCT-CUTOVER.md)** for full Nginx commands.

Quick summary:

```bash
# Home Care UI
sudo cp Scheduler.Client/eXtream-scheduler/deploy/nginx-homecare.caresynx.com.conf \
  /etc/nginx/sites-available/homecare.caresynx.com
sudo ln -sf /etc/nginx/sites-available/homecare.caresynx.com /etc/nginx/sites-enabled/

# Home Care API (replace old api.caresynx.com site)
sudo rm -f /etc/nginx/sites-enabled/caresynx-api
sudo cp Scheduler.API/deploy/nginx-api.homecare.caresynx.com.conf \
  /etc/nginx/sites-available/api.homecare.caresynx.com
sudo ln -sf /etc/nginx/sites-available/api.homecare.caresynx.com /etc/nginx/sites-enabled/

sudo rm -f /etc/nginx/sites-enabled/caresynx
sudo nginx -t && sudo systemctl reload nginx

sudo certbot --nginx -d homecare.caresynx.com
sudo certbot --nginx -d api.homecare.caresynx.com
```

## 5. Deploy (in order)

1. **Deploy Backend API to VPS**
2. **Deploy Home Care UI to VPS**

## 6. Verify

| URL | Expected |
|-----|----------|
| https://homecare.caresynx.com/login | Home Care login |
| https://api.homecare.caresynx.com/swagger | API Swagger |

Marketing site is deployed from the separate **caresynx-marketing** repo.
