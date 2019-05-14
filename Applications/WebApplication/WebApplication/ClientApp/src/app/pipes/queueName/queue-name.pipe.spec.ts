import { QueueNamePipe } from './queue-name.pipe';

describe('QueueNamePipe', () => {
  it('create an instance', () => {
    const pipe = new QueueNamePipe();
    expect(pipe).toBeTruthy();
  });
});
