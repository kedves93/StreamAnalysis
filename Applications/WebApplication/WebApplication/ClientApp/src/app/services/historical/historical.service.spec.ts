import { TestBed } from '@angular/core/testing';

import { HistoricalService } from './historical.service';

describe('HistoricalService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: HistoricalService = TestBed.get(HistoricalService);
    expect(service).toBeTruthy();
  });
});
