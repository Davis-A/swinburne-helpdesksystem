import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { HttpClient } from '@angular/common/http';
import { GetHelpdesksResponse } from '../data/responses/helpdesk/get-all-response';
import { GetHelpdeskResponse } from '../data/responses/configuration/get-response';
import { GetUnitsByHelpdeskIdResponse } from '../data/responses/units/get-by-help-id.response';
import { CheckInRequest } from '../data/requests/check-in/chek-in-request';
import { CheckInResponse } from '../data/responses/helpdesk/check-in.response';
import { GetCheckInsResponse } from '../data/responses/helpdesk/get-check-ins.response';
import { CheckOutRequest } from '../data/requests/check-in/check-out-request';
import { GetQueueItemsByHelpdeskIDResponse } from '../data/responses/helpdesk/get-queue-items-by-helpdesk-response';

@Injectable()
export class HelpdeskService {

  constructor(private client: HttpClient) {
  }

  /**
   * Returns all helpdesks
   * @return Helpdesk[]
   */
  getHelpdesks() {
    return this.client.get<GetHelpdesksResponse>("/api/helpdesk/")
  }

  /**
   * Returns all active helpdesks
   * @return Helpdesk[]
   */
  getActiveHelpdesks() {
    return this.client.get<GetHelpdesksResponse>("/api/helpdesk/active")
  }

  /**
   * Returns a specific helpdesk
   * @param id Unique Id of the helpdesk to return
   * @return GetHelpdeskResponse
   */
  getHelpdeskById(id: number) {
    return this.client.get<GetHelpdeskResponse>("/api/helpdesk/" + id);
  }

  /**
   * Used to retreive the list of active units for the helpdesk
   * @param id The id of the helpdesk
   * @returns GetUnitsByHelpdeskIdResponse
   */
  getActiveUnitsByHelpdeskId(id: number) {
    return this.client.get<GetUnitsByHelpdeskIdResponse>("/api/units/helpdesk/" + id + '/active');
  }

  /**
   * This function is used to check in a student
   * @param request the check in information
   * @returns the response that indicates success
   */
  checkIn(request: CheckInRequest) {
    return this.client.post<CheckInResponse>("/api/checkin/", request);
  }

  /**
   * This function is used to check out a student
   * @param id the check in id
   * @param request request indicating the the checkout is not forced
   */
  checkOut(id: number, request: CheckOutRequest) {
    return this.client.post<CheckInResponse>("/api/checkin/" + id , request);
  }

  /**
   * Used to get the check ins for the helpdesk id required
   * @param id The id of the helpdesk
   * @returns The response with the check ins if they are any
   */
  getCheckInsByHelpdesk(id: number) {
    return this.client.get<GetCheckInsResponse>("/api/checkin/" + id);
  }
  
  /**
   * Used to retreive all active queue items for a helpdesk
   * @param id the id of the helpdesk
   * @returns the response that contains an array of helpdesk QueueItems
   */
  getQueueItemsByHelpdesk(id: number) {
    return this.client.get<GetQueueItemsByHelpdeskIDResponse>("/api/queue/helpdesk/" + id);
  }
}
