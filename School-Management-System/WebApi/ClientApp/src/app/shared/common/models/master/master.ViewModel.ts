import { DistrictViewModel } from "./district.ViewModel";

export interface ProvinceViewModel{
    id:number;
    name:string;
    districts:DistrictViewModel[];
}