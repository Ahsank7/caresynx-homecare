# VPS cutover: Home Care subdomains

Home Care product deploys from **caresynx-homecare**. Marketing deploys from **caresynx-marketing**.

## DNS (GoDaddy) — Home Care only

| Type | Name | Value |
|------|------|--------|
| A | `homecare` | VPS IP |
| A | `api.homecare` | VPS IP |

For `caresynx.com` / `www`, see the **caresynx-marketing** repo.

Remove or stop using:

- `api` → old Home Care API host (`api.caresynx.com`)
- `www` CNAME → CloudFront

## Nginx sites (Home Care)

```bash
# Home Care UI — homecare.caresynx.com
sudo cp Scheduler.Client/eXtream-scheduler/deploy/nginx-homecare.caresynx.com.conf \
  /etc/nginx/sites-available/homecare.caresynx.com
sudo ln -sf /etc/nginx/sites-available/homecare.caresynx.com /etc/nginx/sites-enabled/

# Home Care API — api.homecare.caresynx.com (replace old api.caresynx.com site)
sudo rm -f /etc/nginx/sites-enabled/caresynx-api /etc/nginx/sites-enabled/api.caresynx.com
sudo cp Scheduler.API/deploy/nginx-api.homecare.caresynx.com.conf \
  /etc/nginx/sites-available/api.homecare.caresynx.com
sudo ln -sf /etc/nginx/sites-available/api.homecare.caresynx.com /etc/nginx/sites-enabled/

# Remove old combined UI site if present
sudo rm -f /etc/nginx/sites-enabled/caresynx /etc/nginx/sites-enabled/caresynx.com.bak

sudo nginx -t && sudo systemctl reload nginx
```

## TLS (Home Care)

```bash
sudo certbot --nginx -d homecare.caresynx.com
sudo certbot --nginx -d api.homecare.caresynx.com
```

## Deploy folders

```bash
sudo mkdir -p /var/www/homecare /opt/caresynx/publish
sudo chown -R root:root /var/www/homecare
```

## GitHub Actions (after DNS + Nginx)

1. **Deploy Backend API to VPS**
2. **Deploy Home Care UI to VPS**

Marketing deploy is in the **caresynx-marketing** repo.

Optional variables (defaults are set in workflows):

| Variable | Default |
|----------|---------|
| `REACT_APP_API_URL` | `https://api.homecare.caresynx.com/api/` |
| `VPS_UI_DIR` | `/var/www/homecare` |
| `VPS_MARKETING_DIR` | `/var/www/caresynx-marketing` |
| `VPS_UI_HEALTHCHECK_URL` | `https://homecare.caresynx.com/` |
| `VPS_HEALTHCHECK_URL` | `https://api.homecare.caresynx.com/swagger/index.html` |

## Expected URLs

| URL | Serves |
|-----|--------|
| https://homecare.caresynx.com/login | Home Care app |
| https://api.homecare.caresynx.com/swagger | Home Care API |
