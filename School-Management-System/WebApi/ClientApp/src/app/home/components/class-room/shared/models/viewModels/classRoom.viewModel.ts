import { SectionViewModel } from "./section.viewModel";

export interface ClassRoomViewModel {
    id: string;
    name: string;
    roomNumber: string;
    academicYear: string;
    sections: SectionViewModel[];
}