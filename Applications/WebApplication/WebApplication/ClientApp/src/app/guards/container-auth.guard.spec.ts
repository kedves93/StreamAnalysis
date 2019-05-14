import { TestBed, async, inject } from '@angular/core/testing';

import { ContainerAuthGuard } from './container-auth.guard';

describe('ContainerAuthGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ContainerAuthGuard]
    });
  });

  it('should ...', inject([ContainerAuthGuard], (guard: ContainerAuthGuard) => {
    expect(guard).toBeTruthy();
  }));
});
