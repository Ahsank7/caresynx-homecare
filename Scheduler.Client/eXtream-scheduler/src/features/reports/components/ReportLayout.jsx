import React from 'react';
import { Button, Group, Paper, Text, ThemeIcon } from '@mantine/core';
import { IconDownload, IconPrinter } from '@tabler/icons';

/**
 * ReportLayout Component
 * Provides a standardized A4-sized layout for all reports with download and print functionality
 * 
 * @param {Object} props
 * @param {string} props.title - Report title
 * @param {ReactNode} props.children - Report content
 * @param {string} props.reportType - Type of report (for filename)
 * @param {Object} props.headerInfo - Additional header information
 * @param {ReactNode} props.customActions - Custom action buttons
 */
export default function ReportLayout({ 
  title, 
  children, 
  reportType = 'report',
  headerInfo = null,
  customActions = null 
}) {
  const reportStyles = `
    .report-shell {
      position: relative;
    }

    .report-preview-stage {
      padding: 28px;
      border-radius: 24px;
      background:
        radial-gradient(circle at top left, rgba(34, 139, 230, 0.12), transparent 32%),
        radial-gradient(circle at top right, rgba(64, 192, 87, 0.08), transparent 26%),
        linear-gradient(180deg, #f8fbff 0%, #eef4f8 100%);
    }

    .report-preview-frame {
      overflow: hidden;
      border: 1px solid #d9e6f2;
      border-radius: 22px;
      background: #ffffff;
      box-shadow: 0 24px 60px rgba(20, 40, 80, 0.14);
    }

    .report-body {
      padding: 32px 36px 40px;
      color: #17324d;
      font-family: Georgia, "Times New Roman", serif;
    }

    .report-header {
      position: relative;
      margin-bottom: 28px;
      padding-bottom: 20px;
      border-bottom: 1px solid #dbe7f3;
    }

    .report-header::after {
      content: "";
      position: absolute;
      left: 0;
      bottom: -1px;
      width: 120px;
      height: 4px;
      border-radius: 999px;
      background: linear-gradient(90deg, #1c7ed6 0%, #5c7cfa 100%);
    }

    .report-kicker {
      margin-bottom: 8px;
      font-family: "Segoe UI", Arial, sans-serif;
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.18em;
      text-transform: uppercase;
      color: #5b7a99;
    }

    .report-title {
      margin: 0;
      font-size: 34px;
      line-height: 1.1;
      font-weight: 700;
      color: #10243a;
    }

    .report-subtitle {
      margin-top: 10px;
      max-width: 640px;
      font-family: "Segoe UI", Arial, sans-serif;
      font-size: 14px;
      line-height: 1.6;
      color: #5d7289;
    }

    .report-meta {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
      gap: 12px;
      margin-top: 22px;
      font-family: "Segoe UI", Arial, sans-serif;
    }

    .meta-item {
      padding: 14px 16px;
      border: 1px solid #dbe7f3;
      border-radius: 16px;
      background: linear-gradient(180deg, #f9fcff 0%, #f1f7fb 100%);
    }

    .meta-label {
      display: block;
      margin-bottom: 5px;
      font-size: 10px;
      font-weight: 700;
      letter-spacing: 0.14em;
      text-transform: uppercase;
      color: #6b8299;
    }

    .meta-value {
      display: block;
      font-size: 15px;
      font-weight: 700;
      color: #17324d;
    }

    .report-content {
      margin-top: 26px;
      font-family: "Segoe UI", Arial, sans-serif;
    }

    .section-title {
      margin: 30px 0 16px;
      font-size: 18px;
      font-weight: 700;
      color: #14314d;
    }

    .section-caption {
      margin: -8px 0 18px;
      font-size: 13px;
      color: #70859b;
    }

    .report-stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
      gap: 14px;
      margin-bottom: 12px;
    }

    .stat-card {
      position: relative;
      overflow: hidden;
      padding: 18px 18px 20px;
      border-radius: 18px;
      color: #fff;
      box-shadow: 0 16px 30px rgba(32, 56, 84, 0.14);
    }

    .stat-card::before {
      content: "";
      position: absolute;
      inset: 0;
      background: linear-gradient(140deg, rgba(255, 255, 255, 0.16), rgba(255, 255, 255, 0));
      pointer-events: none;
    }

    .stat-label {
      position: relative;
      font-size: 11px;
      font-weight: 700;
      letter-spacing: 0.12em;
      text-transform: uppercase;
      opacity: 0.82;
    }

    .stat-value {
      position: relative;
      margin-top: 12px;
      font-size: 30px;
      line-height: 1;
      font-weight: 800;
      letter-spacing: -0.03em;
    }

    .report-card {
      margin: 14px 0;
      padding: 18px 20px;
      border: 1px solid #dbe7f3;
      border-radius: 18px;
      background: linear-gradient(180deg, #ffffff 0%, #f7fbff 100%);
    }

    .report-empty {
      padding: 38px 24px;
      border: 1px dashed #b9ccdd;
      border-radius: 18px;
      background: #f8fbfe;
      text-align: center;
      color: #72859a;
    }

    table {
      width: 100%;
      border-collapse: separate;
      border-spacing: 0;
      margin: 18px 0 0;
      overflow: hidden;
      border: 1px solid #dbe7f3;
      border-radius: 18px;
      background: #ffffff;
    }

    th, td {
      padding: 14px 16px;
      text-align: left;
      border-bottom: 1px solid #ebf1f6;
    }

    th {
      background: #f4f8fc;
      color: #48627a;
      font-size: 11px;
      font-weight: 800;
      letter-spacing: 0.12em;
      text-transform: uppercase;
    }

    td {
      color: #20384f;
      font-size: 14px;
      vertical-align: top;
    }

    tbody tr:nth-child(even) td {
      background: #fbfdff;
    }

    tbody tr:last-child td {
      border-bottom: none;
    }

    .report-status-pill {
      display: inline-flex;
      align-items: center;
      padding: 5px 12px;
      border-radius: 999px;
      font-size: 11px;
      font-weight: 800;
      letter-spacing: 0.05em;
      text-transform: uppercase;
    }

    @page {
      size: A4;
      margin: 0;
    }

    @media print {
      .report-preview-stage {
        padding: 0;
        background: transparent;
      }

      .report-preview-frame {
        border: none;
        border-radius: 0;
        box-shadow: none;
      }

      .report-body {
        padding: 20mm;
      }

      html, body {
        width: 210mm;
        height: 297mm;
      }

      body {
        margin: 0;
        padding: 0;
        background: white;
      }

      .no-print {
        display: none !important;
      }
    }
  `;

  const handleDownloadPDF = () => {
    // Create a new window with the report content
    const printWindow = window.open('', '_blank');
    
    if (!printWindow) {
      alert('Please allow pop-ups to download the report');
      return;
    }

    const reportContent = document.getElementById('report-content').innerHTML;
    
    const htmlContent = `
      <!DOCTYPE html>
      <html>
        <head>
          <meta charset="utf-8">
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
          <title>${title}</title>
          <style>
            * {
              margin: 0;
              padding: 0;
              box-sizing: border-box;
            }

            body {
              font-family: "Segoe UI", Arial, sans-serif;
              line-height: 1.6;
              color: #17324d;
              background: white;
            }

            .a4-container {
              width: 210mm;
              min-height: 297mm;
              background: white;
            }
            
            ${reportStyles}

            .print-button {
              background: #228be6;
              color: white;
              border: none;
              padding: 12px 24px;
              font-size: 16px;
              font-weight: 600;
              border-radius: 8px;
              cursor: pointer;
              display: flex;
              align-items: center;
              gap: 8px;
              margin: 30px auto;
              transition: background 0.2s;
            }

            .print-button:hover {
              background: #1c7ed6;
            }

            .print-instructions {
              background: #e7f5ff;
              border: 1px solid #339af0;
              border-radius: 8px;
              padding: 20px;
              margin-top: 20px;
            }

            .print-instructions p {
              margin: 8px 0;
              color: #1864ab;
            }

            .print-instructions strong {
              color: #1864ab;
              font-weight: 700;
            }
          </style>
        </head>
        <body>
          <div class="a4-container">
            ${reportContent}
          </div>
          
          <div class="no-print">
            <button class="print-button" onclick="window.print()">
              <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <path d="M6 9V2h12v7"></path>
                <path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"></path>
                <path d="M6 14h12v8H6z"></path>
              </svg>
              Print / Save as PDF
            </button>
            <div class="print-instructions">
              <p><strong>How to Download as PDF:</strong></p>
              <p>1. Click the "Print / Save as PDF" button above</p>
              <p>2. In the print dialog, select "Save as PDF" as the destination</p>
              <p>3. Ensure paper size is set to A4 (210 x 297 mm)</p>
              <p>4. Set margins to "Default" or "Minimum" for best results</p>
              <p>5. Click "Save" to download your PDF report</p>
            </div>
          </div>
        </body>
      </html>
    `;
    
    printWindow.document.write(htmlContent);
    printWindow.document.close();
  };

  return (
    <div className="report-shell">
      <style>{reportStyles}</style>
      <Paper
        p="lg"
        mb="lg"
        radius="xl"
        withBorder
        style={{
          borderColor: '#dbe7f3',
          background: 'linear-gradient(180deg, #ffffff 0%, #f7fbff 100%)',
          boxShadow: '0 12px 28px rgba(15, 35, 70, 0.08)',
        }}
      >
        <Group position="apart">
          <Group spacing="md" noWrap align="flex-start">
            <ThemeIcon size={48} radius="xl" variant="light" color="blue">
              <IconPrinter size={22} />
            </ThemeIcon>
            <div>
              <Text size="xs" weight={800} color="dimmed" style={{ letterSpacing: '0.12em', textTransform: 'uppercase' }} mb={4}>
                Report Actions
              </Text>
              <Text size="lg" weight={700} color="dark" mb={2}>
                Ready for print and PDF
              </Text>
              <Text size="sm" color="dimmed">
                Review the polished A4 preview below, then export it when everything looks right.
              </Text>
            </div>
          </Group>
          <Group>
            {customActions}
            <Button
              variant="filled"
              color="blue"
              radius="xl"
              size="md"
              leftIcon={<IconDownload size={16} />}
              onClick={handleDownloadPDF}
            >
              Download as PDF
            </Button>
          </Group>
        </Group>
      </Paper>

      <div className="report-preview-stage">
        <Paper 
          className="report-preview-frame"
          p={0}
          style={{
            maxWidth: '210mm',
            margin: '0 auto',
            background: 'white'
          }}
        >
          <div id="report-content" className="report-body">
            <div className="report-header">
              <div className="report-kicker">{reportType.replace(/-/g, ' ')} report</div>
              <div className="report-title">{title}</div>
              <div className="report-subtitle">
                Structured for screen review and clean A4 export, with summary metrics and detailed records in one place.
              </div>
              {headerInfo && (
                <div className="report-meta">
                  {Object.entries(headerInfo).map(([key, value]) => (
                    <div key={key} className="meta-item">
                      <span className="meta-label">{key}</span>
                      <span className="meta-value">{value}</span>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="report-content">
              {children}
            </div>
          </div>
        </Paper>
      </div>
    </div>
  );
}

