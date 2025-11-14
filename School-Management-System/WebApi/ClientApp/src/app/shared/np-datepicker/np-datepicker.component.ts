import { Component, EventEmitter, Input, Output, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import DateConverter from '@remotemerge/nepali-date-converter';

@Component({
  selector: 'app-np-datepicker',
  templateUrl: './np-datepicker.component.html',
  styleUrls: ['./np-datepicker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => NpDatepickerComponent),
      multi: true
    }
  ]
})
export class NpDatepickerComponent implements ControlValueAccessor, OnInit {
  @Input() placeholder: string = 'Select Nepali Date';
  @Input() minDate?: { year: number; month: number; day: number };
  @Input() maxDate?: { year: number; month: number; day: number };
  @Input() language: 'ne' | 'en' = 'ne';
  @Input() unicodeDate: boolean = true;

  @Output() dateChange = new EventEmitter<{ bs: string, ad: string }>();

  showCalendar = false;
  selectedBS: string = '';
  displayDate: string = '';
  currentYear!: number;
  currentMonth!: number;

  years: number[] = [];
  months: { bs: string; en: string }[] = [
    { bs: 'बैशाख', en: 'Baisakh' },
    { bs: 'जेठ', en: 'Jestha' },
    { bs: 'असार', en: 'Asar' },
    { bs: 'श्रावण', en: 'Shrawan' },
    { bs: 'भाद्र', en: 'Bhadra' },
    { bs: 'आश्विन', en: 'Ashwin' },
    { bs: 'कार्तिक', en: 'Kartik' },
    { bs: 'मंसिर', en: 'Mangsir' },
    { bs: 'पौष', en: 'Poush' },
    { bs: 'माघ', en: 'Magh' },
    { bs: 'फाल्गुण', en: 'Falgun' },
    { bs: 'चैत्र', en: 'Chaitra' }
  ];

  calendar: (number | null)[][] = [];

  private onChange = (_: any) => {};
  private onTouched = () => {};

  ngOnInit(): void {
    // Initialize to today in BS
    const nowBS = new DateConverter(new Date().toISOString().split('T')[0]).toBs();
    this.currentYear = nowBS.year;
    this.currentMonth = nowBS.month;
    this.generateYears();
    this.generateCalendar();
  }

  writeValue(obj: any): void {
    if (obj) {
      this.selectedBS = obj;
      this.displayDate = this.unicodeDate ? this.toNepaliDigits(obj) : obj;
      const parts = obj.split('-').map((x: string) => parseInt(x, 10));
      if (parts.length === 3) {
        this.currentYear = parts[0];
        this.currentMonth = parts[1];
        this.generateCalendar();
      }
    }
  }

  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }

  toggleCalendar(): void {
    this.showCalendar = !this.showCalendar;
  }

  selectYearMonth(year: number, month: number): void {
    this.currentYear = year;
    this.currentMonth = month;
    this.generateCalendar();
  }

  selectDate(day: any): void {
    if (!day) return;

    const bsString = `${this.currentYear}-${String(this.currentMonth).padStart(2,'0')}-${String(day).padStart(2,'0')}`;
    const convToAd = new DateConverter(bsString).toAd();
    const adString = `${convToAd.year}-${String(convToAd.month).padStart(2,'0')}-${String(convToAd.date).padStart(2,'0')}`;

    this.selectedBS = bsString;
    this.displayDate = this.unicodeDate ? this.toNepaliDigits(bsString) : bsString;

    this.onChange(bsString);
    this.onTouched();
    this.dateChange.emit({ bs: bsString, ad: adString });

    this.showCalendar = false;
  }

  prevMonth(): void {
    if (this.currentMonth === 1) {
      this.currentYear--;
      this.currentMonth = 12;
    } else {
      this.currentMonth--;
    }
    this.generateCalendar();
  }

  nextMonth(): void {
    if (this.currentMonth === 12) {
      this.currentYear++;
      this.currentMonth = 1;
    } else {
      this.currentMonth++;
    }
    this.generateCalendar();
  }

  selectToday(): void {
    const nowAd = new Date();
    const bs = new DateConverter(nowAd.toISOString().split('T')[0]).toBs();
    this.currentYear = bs.year;
    this.currentMonth = bs.month;
    this.selectDate(bs.date);
  }

  generateYears(): void {
    const minY = this.minDate?.year ?? 2070;
    const maxY = this.maxDate?.year ?? 2099;
    this.years = [];
    for (let y = minY; y <= maxY; y++) {
      this.years.push(y);
    }
  }

  // generateCalendar(): void {
  //   // first day of the month in BS
  //   const firstDayAd = new DateConverter(`${this.currentYear}-${String(this.currentMonth).padStart(2,'0')}-01`).toAd();
  //   const firstDayJs = new Date(firstDayAd.year, firstDayAd.month - 1, firstDayAd.date);
  //   const startDayIndex = firstDayJs.getDay();

  //   // We don't know days in BS month easily via this library so approximate by searching next month first day and subtracting
  //   const nextMonth = this.currentMonth === 12 ? 1 : this.currentMonth + 1;
  //   const nextYear = this.currentMonth === 12 ? this.currentYear + 1 : this.currentYear;
  //   const nextMonthAd = new DateConverter(`${nextYear}-${String(nextMonth).padStart(2,'0')}-01`).toAd();
  //   const nextMonthJs = new Date(nextMonthAd.year, nextMonthAd.month - 1, nextMonthAd.date);
  //   const daysInJs = Math.ceil((nextMonthJs.getTime() - firstDayJs.getTime()) / (1000 * 60 * 60 * 24));
  //   const daysInMonth = daysInJs;

  //   const weeks: (number | null)[][] = [];
  //   let week: (number | null)[] = [];

  //   let i = 0;
  //   for (; i < startDayIndex; i++) {
  //     week.push(null);
  //   }
  //   for (let d = 1; d <= daysInMonth; d++) {
  //     week.push(d);
  //     if (week.length === 7) {
  //       weeks.push(week);
  //       week = [];
  //     }
  //   }
  //   if (week.length) weeks.push(week);

  //   this.calendar = weeks;
  // }

  generateCalendar(): void {
  try {
    // Convert 1st day of current BS month to AD
    const firstDayAd = new DateConverter(
      `${this.currentYear}-${String(this.currentMonth).padStart(2, '0')}-01`
    ).toAd();
    const firstDayJs = new Date(firstDayAd.year, firstDayAd.month - 1, firstDayAd.date);
    const startDayIndex = firstDayJs.getDay();

    // Calculate next month’s 1st day safely
    let nextMonth = this.currentMonth + 1;
    let nextYear = this.currentYear;
    if (nextMonth > 12) {
      nextMonth = 1;
      nextYear++;
    }

    let daysInMonth = 30; // fallback in case of invalid nextMonth conversion

    try {
      const nextMonthAd = new DateConverter(
        `${nextYear}-${String(nextMonth).padStart(2, '0')}-01`
      ).toAd();
      const nextMonthJs = new Date(nextMonthAd.year, nextMonthAd.month - 1, nextMonthAd.date);

      // Difference gives number of days in BS month
      const diffDays = Math.round(
        (nextMonthJs.getTime() - firstDayJs.getTime()) / (1000 * 60 * 60 * 24)
      );

      // Only accept if in a valid range (28–32 days)
      if (diffDays >= 28 && diffDays <= 32) {
        daysInMonth = diffDays;
      }
    } catch {
      // If conversion fails (out of range BS date), keep fallback 30
      daysInMonth = 30;
    }

    // Now generate the weeks and fill the calendar
    const weeks: (number | null)[][] = [];
    let week: (number | null)[] = [];

    for (let i = 0; i < startDayIndex; i++) week.push(null);

    for (let d = 1; d <= daysInMonth; d++) {
      week.push(d);
      if (week.length === 7) {
        weeks.push(week);
        week = [];
      }
    }

    if (week.length) weeks.push(week);
    this.calendar = weeks;
  } catch (err) {
    console.error('Nepali date conversion failed:', err);
    // fallback: empty calendar
    this.calendar = [];
  }
}


  isDisabled(day: number): boolean {
    if (!day) return true;
    const bsStr = `${this.currentYear}-${String(this.currentMonth).padStart(2,'0')}-${String(day).padStart(2,'0')}`;
    const adObj = new DateConverter(bsStr).toAd();
    const currentAd = new Date(adObj.year, adObj.month - 1, adObj.date);

    if (this.minDate) {
      const minAdObj = new DateConverter(`${this.minDate.year}-${String(this.minDate.month).padStart(2,'0')}-${String(this.minDate.day).padStart(2,'0')}`).toAd();
      const minAd = new Date(minAdObj.year, minAdObj.month - 1, minAdObj.date);
      if (currentAd < minAd) return true;
    }
    if (this.maxDate) {
      const maxAdObj = new DateConverter(`${this.maxDate.year}-${String(this.maxDate.month).padStart(2,'0')}-${String(this.maxDate.day).padStart(2,'0')}`).toAd();
      const maxAd = new Date(maxAdObj.year, maxAdObj.month - 1, maxAdObj.date);
      if (currentAd > maxAd) return true;
    }
    return false;
  }

  toNepaliDigits(str: string): string {
    const digits = ['०','१','२','३','४','५','६','७','८','९'];
    return str.replace(/\d/g, d => digits[parseInt(d, 10)]);
  }
}
