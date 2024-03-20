import { Component, ViewChild } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { AuctionService } from '../../services/auction.service';
import { Router } from '@angular/router';
import { GroupedReport } from '../../models/grouped-report.model';
import { Report } from '../../models/report.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
})
export class AdminComponent {
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  showAllReportsTable = false;
  showGroupedReportsTable = false;
  groupedReports: GroupedReport[] = [];
  selectedAuctionIndex: number | null = null;
  selectedAuctionId: number | null = null;
  secondTableData: any[] = [];
  dataSource = new MatTableDataSource<any>();
  totalReports = 0;
  pageSize = 5;
  currentPage = 1;
  secondTotalReports = 0;
  secondPageSize = 5;
  secondCurrentPage = 1;
  displayedColumns: string[] = [
    'id',
    'auction_Id',
    'userName',
    'report_Message',
    'report_Time',
    'remove',
  ];

  constructor(
    private adminService: AdminService,
    private auctionService: AuctionService,
    private router: Router
  ) {}

  ngAfterViewInit() {
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  toggleAllReportsTable() {
    this.showAllReportsTable = !this.showAllReportsTable;
    if (this.showAllReportsTable) {
      this.loadReports(this.currentPage, this.pageSize);
    }
  }

  loadReports(page: number, pageSize: number): void {
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;

    this.adminService.GetAllReports().subscribe((reports) => {
      this.totalReports = reports.length;
      this.dataSource.data = reports.slice(startIndex, endIndex);
    });
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadReports(this.currentPage, this.pageSize);
  }

  toggleGroupedReportsTable(): void {
    this.showGroupedReportsTable = !this.showGroupedReportsTable;
    if (this.showGroupedReportsTable) {
      this.loadGroupedReports(this.secondCurrentPage, this.secondPageSize);
    }
  }

  loadGroupedReports(page: number, pageSize: number): void {
    const startIndex = (page - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    this.adminService.GetAllGroupedReports().subscribe((reports) => {
      const transformedReports = [];
      for (const auctionId in reports) {
        if (reports.hasOwnProperty(auctionId)) {
          const auctionReports = reports[auctionId];
          const reportCount = Array.isArray(auctionReports)
            ? auctionReports.length
            : 0;
          transformedReports.push({ auctionId: +auctionId, reportCount });
        }
      }
      this.groupedReports = transformedReports.slice(startIndex, endIndex);
    });
  }

  openAuction(auctionId: number): void {
    this.router.navigate(['/auction-bid', auctionId]);
  }

  openAuctionReports(auctionId: number, index: number): void {
    if (this.selectedAuctionIndex === index) {
      this.selectedAuctionIndex = null;
      this.secondTableData = [];
    } else {
      this.selectedAuctionId = auctionId;
      this.selectedAuctionIndex = index;
      this.adminService.GetAuctionReports(auctionId).subscribe((reports) => {
        this.secondTableData = reports;
      });
    }
  }

  closeAuctionReports(): void {
    this.selectedAuctionId = null;
    this.secondTableData = [];
  }

  onSecondPageChange(event: PageEvent): void {
    this.secondCurrentPage = event.pageIndex + 1;
    this.loadGroupedReports(this.secondCurrentPage, this.secondPageSize);
  }

  deleteReport(reportId: number) {
    this.adminService.DeleteReport(reportId).subscribe(
      () => {
        console.log('Report was deleted');
        if (this.showAllReportsTable) {
          this.loadReports(this.currentPage, this.pageSize);
        } else if (
          this.showGroupedReportsTable &&
          this.selectedAuctionId !== null
        ) {
          this.loadGroupedReports(this.secondCurrentPage, this.secondPageSize);
          this.openAuctionReports(
            this.selectedAuctionId,
            this.selectedAuctionIndex!
          );
        }
      },
      (error) => {
        console.error('Error deleting report:', error);
      }
    );
  }
}
