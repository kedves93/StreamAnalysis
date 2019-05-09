import { Subscription } from 'rxjs';

export class TopicData {
    topic: string;
    value: string;
    measurement: string;
    icon: string;
    lastUpdate: string;
    lastUpdateSubscription: Subscription;

    constructor(topic: string, value: string = 'N/A', measurement: string = 'N/A', icon: string = '', lastUpdate: string = 'No update') {
        this.topic = topic;
        this.value = value;
        this.measurement = measurement;
        this.icon = icon;
        this.lastUpdate = lastUpdate;
        this.lastUpdateSubscription = new Subscription();
    }
}
