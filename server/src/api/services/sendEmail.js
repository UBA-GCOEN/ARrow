import nodemailer from "nodemailer";
import * as dotenv from "dotenv";
dotenv.config();

const SMTP_MAIL = process.env.SMTP_MAIL;
const SMTP_PASSWORD = process.env.SMTP_PASSWORD;

const Transporter = nodemailer.createTransport({
  service: "gmail",
  auth: { user: SMTP_MAIL, pass: SMTP_PASSWORD },
});

const sendWelcomeMail = async (email) => {
  let info = await Transporter.sendMail({
    from: SMTP_MAIL,
    to: email,
    subject: `Welcome to ARrow!!`,

    html: `<b>Hello!</b>,<br/><br/>
      Thank you for registering at ARrow!<br />
      We're excited to have you on board. Check out the ARrow application and its amazing AR Feature: <br/>
      <b>https://github.com/UBA-GCOEN/ARrow</b><br/><br/>

      Please complete the onboarding process on ARrow app if you haven't.<br/><br/>

      If you encounter any issues or have any questions, please don't hesitate to reach out to our support team at <b>stichhub.office@gmail.com</b> . We're here to help!<br/><br/>

      We're thrilled to have you on board.<br/>
      Thank you for choosing ARrow!<br/><br/>
      
      Best regards,<br/>
      The ARrow Team<br/>`,
  });
};

export default sendWelcomeMail;
