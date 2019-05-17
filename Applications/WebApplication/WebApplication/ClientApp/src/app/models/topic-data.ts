import { Subscription, Observable, Subject } from 'rxjs';

export class TopicData {
    topic: string;
    stream: Subject<string>;
    measurement: string;

    constructor(topic: string, stream: Subject<string> = new Subject<string>(), measurement: string = 'N/A') {
        this.topic = topic;
        this.stream = stream;
        this.measurement = measurement;
    }
}
