import { Component, OnInit } from '@angular/core';
import { NicknameService } from './nickname.service';
import { ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Nickname } from 'src/app/data/DTOs/nickname.dto';
import { EditStudentNicknameRequest } from 'src/app/data/requests/student/edit-student-nickname-request';

@Component({
  selector: 'app-admin-nicknames',
  templateUrl: './nicknames.component.html'
})

export class NicknamesComponent implements OnInit {

  public nicknames: Nickname[] = [];
  public editForm: FormGroup = this.builder.group({
    modalEditId: new FormControl(''),
    modalEditNickname: new FormControl('', [Validators.required, Validators.max(20)])
  });

  constructor(private service: NicknameService,
    private route: ActivatedRoute,
    private notifier: NotifierService,
    private builder: FormBuilder) {
  }
  ngOnInit() {
    this.getNicknames();
  }

  getNicknames() {
    this.service.getNickames().subscribe(
      result => {
        this.nicknames = result.nicknames;
      },
      error => {
        if (error.status != 404) {
          this.notifier.notify('error', 'Unable to retreive nicknames, please contact admin');
        }
      }
    );
  }

  setUpEdit(id: number) {
    var nickname = this.nicknames.find(n => n.id == id)

    this.editForm.controls.modalEditId.setValue(nickname.id);
    this.editForm.controls.modalEditNickname.setValue(nickname.nickname);
  }

  generateNickname() {
    this.service.generateNickname().subscribe(
      result => {
        this.editForm.controls.modalEditNickname.setValue(result.nickname);
      },
      error => {
        this.notifier.notify('error', 'Unable to generate nickname, please enter manually or contact admin');
      }
    );
  }

  editNickname() {
    event.preventDefault();

    if (this.editForm.invalid) {
      if ((!this.editForm.controls.modalEditNickname) || (this.editForm.controls.modalEditNickname.value.length > 20)) {
        this.notifier.notify('warning', 'You must enter in a nickame, that has 20 or less characters');
      }
    }

    var request = new EditStudentNicknameRequest();
    request.nickname = this.editForm.controls.modalEditNickname.value;

    this.service.editNickname(this.editForm.controls.modalEditId.value, request).subscribe(
      result => {
        this.getNicknames();
        this.editForm.reset();
        $("#modal-student-edit").modal('hide')
        this.notifier.notify('success', 'Student nickname updated');
      },
      error => {

        if (error.status == 400) {
          this.notifier.notify('warning', 'Nickname already exists please choose another.')
        }
        else {
          this.notifier.notify('error', 'Unable to save nickname, please contact admin');
        }
      }
    );
  }

  closeEdit() {
    this.editForm.reset();
  }
}
