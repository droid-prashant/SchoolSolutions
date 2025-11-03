import { MunicipalityViewModel } from "./municipality.ViewModel";

export interface DistrictViewModel{
    id:number;
    name:string;
    municipalities:MunicipalityViewModel[];
}