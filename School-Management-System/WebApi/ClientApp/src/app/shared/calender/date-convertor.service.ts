import { Injectable } from '@angular/core';
import DateConverter from '@remotemerge/nepali-date-converter';

export interface BsDateParts {
  year: number;
  month: number;
  day: number;
}

export interface AdDateParts {
  year: number;
  month: number;
  day: number;
}

@Injectable({
  providedIn: 'root'
})
export class DateConverterService {
  bsToAd(bs: string | BsDateParts): string {
    const bsString =
      typeof bs === 'string'
        ? bs
        : `${bs.year}-${this.pad(bs.month)}-${this.pad(bs.day)}`;

    const result: any = new DateConverter(bsString).toAd();
    return `${result.year}-${this.pad(result.month)}-${this.pad(result.date)}`;
  }

  adToBs(ad: string | Date): string {
    const adString = ad instanceof Date ? this.formatAd(ad) : ad;

    const result: any = new DateConverter(adString).toBs();
    return `${result.year}-${this.pad(result.month)}-${this.pad(result.date)}`;
  }

  bsToAdParts(bs: string | BsDateParts): AdDateParts {
    const ad = this.bsToAd(bs);
    const [year, month, day] = ad.split('-').map(Number);
    return { year, month, day };
  }

  adToBsParts(ad: string | Date): BsDateParts {
    const bs = this.adToBs(ad);
    const [year, month, day] = bs.split('-').map(Number);
    return { year, month, day };
  }

  formatAd(date: Date): string {
    return `${date.getFullYear()}-${this.pad(date.getMonth() + 1)}-${this.pad(date.getDate())}`;
  }

  private pad(value: number): string {
    return String(value).padStart(2, '0');
  }
}