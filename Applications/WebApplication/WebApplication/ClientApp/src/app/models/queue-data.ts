export class QueueData {
    queue: string;
    value: number;
    timestampEpoch: number;
    measurement: string;

    constructor(queue: string, value: number, timestampEpoch, measurement: string) {
        this.queue = queue;
        this.value = value;
        this.timestampEpoch = timestampEpoch;
        this.measurement = measurement;
    }
}
