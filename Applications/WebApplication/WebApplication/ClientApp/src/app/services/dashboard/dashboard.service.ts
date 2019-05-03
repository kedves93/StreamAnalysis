import { Injectable } from '@angular/core';
import { Subscription, Observable, timer } from 'rxjs';
import { WebsocketService } from '../websocket/websocket.service';
import { take, map, merge } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  public messagesUpdate;

  private messagesSubscription: Subscription;
  private messagesTimerSubscription: Subscription;

  constructor(private websocket: WebsocketService) {
    this.messagesUpdate = 'No update';
    this.messagesSubscription = new Subscription();
    this.messagesTimerSubscription = new Subscription();
  }

  public startMessages(): void {
  }

  public stopMessages(): void {
    this.messagesSubscription.unsubscribe();
  }

  private updateTimer(): Observable<string> {
    const seconds = timer(0, 5 * 1000).pipe(take(12)).pipe(map(x => {
      if (x === 0) {
        return 'Just updated';
      } else {
        return 'updated ' + x * 5 + ' seconds ago';
      }
    }));
    const minutes = timer(60 * 1000, 60 * 1000).pipe(take(59)).pipe(map(x => {
      return 'updated ' + (x + 1) + ' minutes ago';
    }));
    const hours = timer(60 * 60 * 1000, 60 * 60 * 1000).pipe(map(x => {
      return 'updated ' + (x + 1) + ' hours ago';
    }));
    return seconds.pipe(merge(minutes)).pipe(merge(hours));
  }
}
