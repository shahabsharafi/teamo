import { Input, Component, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { IdentityClient, LoginCommand } from '../../identity-service.proxy';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  constructor(private _client: IdentityClient) {

  }

  form: FormGroup = new FormGroup({
    username: new FormControl(''),
    password: new FormControl(''),
  });

  submit() {
    if (this.form.valid) {
      const command = new LoginCommand();
      command.userName = this.form.value.username;
      command.password = this.form.value.password;
      
      this._client.login(command).toPromise().catch((reason: any) => {
        debugger;
        alert(reason.message)
      });
    }
  }
  @Input() error: string | null | undefined;
}
