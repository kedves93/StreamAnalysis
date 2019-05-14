export class QueueData {
    queue: string;
    value: string;
    measurement: string;

    constructor(queue: string, value: string = 'N/A', measurement: string = 'N/A') {
        this.queue = queue;
        this.value = value;
        this.measurement = measurement;
    }
}
