import { StudentCountBySection } from "./studentCountBySection.viewModel";

export interface StudentsByClassViewModel {
  classRoom: string;
  studentsCountBySections: StudentCountBySection[];
}