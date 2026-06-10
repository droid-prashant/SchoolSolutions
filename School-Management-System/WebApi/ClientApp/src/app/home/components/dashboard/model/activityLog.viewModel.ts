import { DashboardActivityViewModel } from './dashboardOverview.viewModel';

export interface DashboardActivityQuery {
  page: number;
  pageSize: number;
  type?: string;
  fromDate?: string;
  toDate?: string;
}

export interface DashboardActivityLogViewModel {
  items: DashboardActivityViewModel[];
  totalCount: number;
  page: number;
  pageSize: number;
}
