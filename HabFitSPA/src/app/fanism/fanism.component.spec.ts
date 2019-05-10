/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { FanismComponent } from './fanism.component';

describe('FanismComponent', () => {
  let component: FanismComponent;
  let fixture: ComponentFixture<FanismComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FanismComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FanismComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
