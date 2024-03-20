import { Component, Inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ReportService } from '../../services/report.service';

@Component({
  selector: 'app-report-modal',
  templateUrl: './report-modal.component.html',
  styleUrl: './report-modal.component.scss',
})
export class ReportModalComponent {
  selectedOption?: string;
  auctionId?: number;
  constructor(
    private dialogRef: MatDialogRef<ReportModalComponent>,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private reportService: ReportService
  ) {
    if (data && data.auctionId) {
      this.auctionId = data.auctionId;
    }
  }

  submit() {
    if (this.selectedOption && this.auctionId) {
      console.log(this.selectedOption);
      console.log(this.auctionId);
      const reportData = {
        report_Message: this.selectedOption,
        auction_Id: this.auctionId.toString(),
      };
      this.dialogRef.close(reportData);
    }
  }
}
